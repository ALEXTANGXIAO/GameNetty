#!/bin/bash

[ -d LubanRelease ] && rm -rf LubanRelease

dotnet build  ../../../luban/src/Luban/Luban.csproj -c Release -o LubanRelease