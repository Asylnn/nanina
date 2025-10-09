#!/bin/sh
mkdir dist
cd server 
dotnet build
cd ../client
npm run build
cd ..
mv server/bin/Debug/net9.0/linux-x64 dist/server
rm -R server/bin
mv client/dist dist/client
cp -R save dist/save
cp .env dist/env
cp config.json dist/config.json
cp baseValues.json dist/baseValues.json