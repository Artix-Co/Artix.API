#!/bin/bash

# Source and destination directories
SOURCE_DIR="/Users/mohammadnazari/RiderProjects/Artix.API/src/Infra/Artix.API.Infra.Sql/Data/Config/Write"
DEST_DIR="/Users/mohammadnazari/RiderProjects/Artix.API/src/Infra/Artix.API.Infra.Sql/Data/Config/Read"

# Ensure the destination directory exists
mkdir -p "$DEST_DIR"

# Find all .cs files in the source directory ending with WriteConfiguration.cs
for file in "$SOURCE_DIR"/*WriteConfiguration.cs; do
    # Check if files exist
    if [[ -f "$file" ]]; then
        # Get the base filename
        filename=$(basename "$file")
        # Replace WriteConfiguration.cs with ReadConfiguration.cs
        new_filename="${filename/WriteConfiguration.cs/ReadConfiguration.cs}"
        # Copy the file to the destination
        cp "$file" "$DEST_DIR/$new_filename"
        
        # Modify the content of the copied file
        sed -i '' 's/namespace Artix.API.Infra.Sql.Data.Config.Write;/namespace Artix.API.Infra.Sql.Data.Config.Read;/g' "$DEST_DIR/$new_filename"
        sed -i '' 's/internal sealed class AppUserWriteConfiguration/internal sealed class AppUserReadConfiguration/g' "$DEST_DIR/$new_filename"
        
        echo "Copied and modified $filename to $new_filename"
    else
        echo "No files ending with WriteConfiguration.cs found in $SOURCE_DIR"
        exit 1
    fi
done

echo "All files have been copied and modified successfully."