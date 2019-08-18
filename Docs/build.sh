#!/bin/bash

# Prerequisites: git, nodejs, markdown-folder-to-html 
# npm -g install markdown-folder-to-html

version=1.2.0

mkdir ./tmp
cd ./tmp

for lang in "KR" "EN"
do
    cd ../$lang
    markdown-folder-to-html
    mv _docs ../tmp/$lang
done

cd ../tmp
echo $PWD
rm -rf .git
git init
git add .
git config user.name tmp
git config user.email tmp@email.com
git commit -m "Commit for zip"
zipName=XlsxParser-${version}-Docs
git archive -o ../../Assets/XlsxParser/$zipName.zip HEAD
cd ../
rm -rf ./tmp
