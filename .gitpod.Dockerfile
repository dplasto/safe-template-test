# FROM gitpod/workspace-dotnet

FROM gitpod/workspace-full:latest

USER gitpod

# Install .NET Core 5.0 SDK binaries on Ubuntu 20.04
# Source: https://dev.to/carlos487/installing-dotnet-core-in-ubuntu-20-04-6jh

RUN mkdir -p /home/gitpod/dotnet && curl -fsSL https://download.visualstudio.microsoft.com/download/pr/a0487784-534a-4912-a4dd-017382083865/be16057043a8f7b6f08c902dc48dd677/dotnet-sdk-5.0.101-linux-x64.tar.gz | tar xz -C /home/gitpod/dotnet
RUN mkdir -p /home/gitpod/dotnet && curl -fsSL https://download.visualstudio.microsoft.com/download/pr/eca743d3-030f-4b1b-bd15-3573091f1c02/f3e464abc31deb7bc2747ed6cc1a8f5c/aspnetcore-runtime-3.1.10-linux-x64.tar.gz | tar xz -C /home/gitpod/dotnet
RUN mkdir -p /home/gitpod/dotnet && curl -fsSL https://download.visualstudio.microsoft.com/download/pr/ec187f12-929e-4aa7-8abc-2f52e147af1d/56b0dbb5da1c191bff2c271fcd6e6394/dotnet-sdk-3.1.404-linux-x64.tar.gz | tar xz -C /home/gitpod/dotnet


ENV DOTNET_ROOT=/home/gitpod/dotnet
ENV PATH=$PATH:/home/gitpod/dotnet

# Install custom tools, runtimes, etc.
# For example "bastet", a command-line tetris clone:
# RUN brew install bastet
#
# More information: https://www.gitpod.io/docs/config-docker/
