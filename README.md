# Icon-Tool
Icon utility to combine and extract ico and cur files


# Planned usecases

## 1. Create an icon from images in current folder
> dotnet icon create [--output icon.ico]

## 2. Create an icon from images in a folder
> dotnet icon create --source folder [--output icon.ico]

## 3. Create an icon from a series of images
> dotnet icon create [--images icon...] --output icon.ico 

## 4. Create a multi-icon from several folders
> dotnet icon create [--iconsets folder...] [--output icon.ico]


## 5. Extract images from an icon
> dotnet icon exctract --source icon.ico [--output folder]