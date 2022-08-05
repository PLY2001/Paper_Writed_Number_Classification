# -*- coding: utf-8 -*-
# @Time : 2022/8/2 13:09
# @Author : PLZ
# @File : NumberClassification.py
# @Software : PyCharm

import torch
from torch import nn
from torch.utils.data import DataLoader
from torchvision import datasets
from torchvision.transforms import ToTensor, Lambda
import pandas as pd


training_data = datasets.MNIST(
    root="data",
    train=True,
    download=True,
    transform=ToTensor(),
    # target_transform=Lambda(lambda y: torch.zeros([10], dtype=torch.float).scatter_(dim=0, index=torch.tensor([y]), value = 1))
)

test_data = datasets.MNIST(
    root="data",
    train=False,
    download=True,
    transform=ToTensor(),
    # target_transform=Lambda(lambda y: torch.zeros([10], dtype=torch.float).scatter_(dim=0, index=torch.tensor([y]), value = 1))
)

if torch.cuda.is_available():
    device = "cuda"
else:
    device = "cpu"

print(f"Using {device} device")

train_dataloader = DataLoader(training_data, batch_size=128)
test_dataloader = DataLoader(test_data, batch_size=128)

class NeuralNetwork(nn.Module):
    def __init__(self):
        super(NeuralNetwork, self).__init__()
        self.flatten = nn.Flatten()
        self.linear_relu_stack = nn.Sequential(
            # nn.Linear(28*28, 32), #模型1
            # nn.ReLU(),#模型1
            # nn.Linear(32, 32),#模型1
            # nn.ReLU(),#模型1
            # nn.Linear(32, 10),#模型1
            nn.Conv2d(1, 16, kernel_size=3, stride=1, padding=1),#模型2
            nn.ReLU(),#模型2
            nn.AdaptiveAvgPool2d((14,14)),  # 模型2
            nn.Conv2d(16, 16, kernel_size=3, stride=1, padding=1),#模型2
            nn.ReLU(),#模型2
            nn.AdaptiveAvgPool2d((2,2)),  #模型2
        )
        self.linear_relu_stack2 = nn.Sequential(
            nn.Linear(2*2*16, 16), #模型2
            nn.ReLU(),#模型2
            nn.Linear(16, 10),#模型2
        )

    def forward(self, x):
        # x = self.flatten(x)#模型1
        logits = self.linear_relu_stack(x)
        logits = logits.view(logits.size(0),logits.size(1)*logits.size(2)*logits.size(3))  # 模型2
        logits = self.linear_relu_stack2(logits) # 模型2
        return logits

model = NeuralNetwork().to(device)

learning_rate = 1e-3

loss_fn = nn.CrossEntropyLoss().to(device)
# optimizer = torch.optim.SGD(model.parameters(), lr=learning_rate)
optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)

def train_loop(dataloader, model, loss_fn, optimizer):
    size = len(dataloader.dataset)
    for batch, (X, y) in enumerate(dataloader):
        # Compute prediction and loss
        X = X.to(device)
        y = y.to(device)
        pred = model(X)
        loss = loss_fn(pred, y)

        # Backpropagation
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()

        if batch % 100 == 0:
            loss, current = loss.item(), batch * len(X)
            print(f"loss: {loss:>7f}  [{current:>5d}/{size:>5d}]")


def test_loop(dataloader, model, loss_fn):
    size = len(dataloader.dataset)
    num_batches = len(dataloader)
    test_loss, correct = 0, 0

    with torch.no_grad():
        for X, y in dataloader:
            X = X.to(device)
            y = y.to(device)
            pred = model(X)
            test_loss += loss_fn(pred, y).item()
            correct += (pred.argmax(1) == y).type(torch.float).sum().item()

    test_loss /= num_batches
    correct /= size
    print(f"Test Error: \n Accuracy: {(100*correct):>0.1f}%, Avg loss: {test_loss:>8f} \n")


epochs = 10
for t in range(epochs):
    print(f"Epoch {t+1}\n-------------------------------")
    train_loop(train_dataloader, model, loss_fn, optimizer)
    test_loop(test_dataloader, model, loss_fn)
print("Done!")

f = open("para.csv","w")
for name, param in model.named_parameters():        #获得神经网络中的所有线性组合层的weight和bias，name里有weight或bias，para是weight矩阵或bias向量
    para = param
    print(para.shape)
    if len(para.shape)==2:
        f.write(name+"\n")
        f.write(str(param.size()) + "\n")
        for i in range(para.shape[0]):
            for j in range(para.shape[1]):
                f.write(str(para[i][j].item())+",")
            f.write("\n")
        f.write("\n")
    elif len(para.shape)==1:
        f.write(name+"\n")
        f.write(str(param.size()) + "\n")
        for i in range(para.shape[0]):
            f.write(str(para[i].item())+"\n")
        f.write("\n")
    elif len(para.shape)==4:
        f.write(name+"\n")
        f.write(str(param.size()) + "\n")
        for i in range(para.shape[0]):
            for j in range(para.shape[1]):
                f.write(str(i)+","+str(j) + "\n")
                for m in range(para.shape[2]):
                    for n in range(para.shape[3]):
                        f.write(str(para[i][j][m][n].item())+",")
                    f.write("\n")
                f.write("\n")
            f.write("\n")
        f.write("\n")