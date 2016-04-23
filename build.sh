#!/bin/sh

xbuild Brief.sln /p:TargetFrameworkVersion="v4.0"
mono ./Brief/bin/Debug/Brief.exe
