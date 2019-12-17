# Icon-Tool
Icon utility to combine and extract ico and cur files


# Planned usecases

## 1. Create an icon from images in current folder
> dotnet icon-tool [--output icon.ico]

## 2. Create an icon from images in a folder
> dotnet icon-tool folder [--output icon.ico]

## 3. Create an icon from a series of images
> dotnet icon-tool [--images icon...] --output icon.ico 

## 4. Create a multi-icon from several folders
> dotnet icon-tool [--iconsets folder...] [--output icon.ico]


## 5. Extract images from an icon
> dotnet icon-tool --exctract icon.ico [--output folder]