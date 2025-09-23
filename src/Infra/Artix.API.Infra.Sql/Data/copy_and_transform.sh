#!/bin/bash

# This script copies all .cs files from Config/Write to Config/Read, preserving the subfolder structure.
# It renames files from *WriteConfiguration.cs to *ReadConfiguration.cs
# It replaces namespace from Artix.API.Infra.Sql.Data.Config.Write.XXX to Artix.API.Infra.Sql.Data.Config.Read.XXX
# It replaces class names from *WriteConfiguration to *ReadConfiguration
# It deletes backup files with -E suffix created by sed on macOS

SRC_DIR="Config/Write"
DST_DIR="Config/Read"

# Check if SRC_DIR exists
if [ ! -d "$SRC_DIR" ]; then
    echo "Error: Source directory $SRC_DIR does not exist!"
    exit 1
fi

# Find all .cs files in SRC_DIR and subdirs
find "$SRC_DIR" -type f -name "*.cs" | while read -r src_file; do
    # Get relative path from SRC_DIR
    rel_path="${src_file#$SRC_DIR/}"
    # Get directory part and base name
    rel_dir=$(dirname "$rel_path")
    base_name=$(basename "$src_file")
    
    # Create target directory if it doesn't exist
    target_dir="$DST_DIR/$rel_dir"
    mkdir -p "$target_dir"
    
    # New file name: replace WriteConfiguration with ReadConfiguration
    new_base_name="${base_name/WriteConfiguration/ReadConfiguration}"
    
    # Target file path
    target_file="$target_dir/$new_base_name"
    
    # Copy the file
    cp "$src_file" "$target_file"
    
    # Replace namespace: Artix.API.Infra.Sql.Data.Config.Write.XXX => Artix.API.Infra.Sql.Data.Config.Read.XXX
    # Use a more precise pattern and ensure it works across systems
    sed -i'' -E 's/Artix\.API\.Infra\.Sql\.Data\.Config\.Write\./Artix\.API\.Infra\.Sql\.Data\.Config\.Read\./g' "$target_file"
    
    # Replace class name: *WriteConfiguration => *ReadConfiguration
    sed -i'' -E 's/WriteConfiguration/ReadConfiguration/g' "$target_file"
    
    # Remove backup file with -E suffix if it exists
    backup_file="$target_file-E"
    if [ -f "$backup_file" ]; then
        rm "$backup_file"
        echo "Removed backup: $backup_file"
    fi
    
    echo "Processed: $src_file -> $target_file"
done