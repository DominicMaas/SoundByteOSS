#!/bin/bash

echo "Building..."
tsc

cd extension

uglifyjs main.js --compress --output main.js

echo "Zipping..."

zip -r upload.zip .
cd ..

echo "Complete!"
