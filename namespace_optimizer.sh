#!/bin/bash

# Namespace Optimizer - Fixed for Artix.API (2025)
# Run from project root: ./namespace_optimizer.sh

BASE_NAMESPACE="Artix.API"
ROOT_DIR="src"

echo "🔥 Namespace Optimizer شروع شد (بر اساس مسیر فولدر) 🔥"
echo

find "$ROOT_DIR" -type f -name "*.cs" | while IFS= read -r file; do
    # مسیر نسبی از src به بعد
    rel_path="${file#$ROOT_DIR/}"
    dir_path=$(dirname "$rel_path")

    # ساخت namespace درست بر اساس ساختار فولدرها
    expected_namespace="$BASE_NAMESPACE.$(echo "$dir_path" | tr '/' '.')"

    # پیدا کردن خط namespace فعلی (حتی اگه فاصله یا تب داشته باشه)
    current_line=$(grep -m 1 '^\s*namespace ' "$file" 2>/dev/null || echo "")

    if [ -z "$current_line" ]; then
        echo "⚠️  No namespace → $rel_path"
        continue
    fi

    # استخراج namespace فعلی
    current_namespace=$(echo "$current_line" | sed -E 's/^\s*namespace\s+([^;[:space:]]+).*/\1/' | xargs)

    if [ "$current_namespace" = "$expected_namespace" ]; then
        echo "✓ $rel_path"
    else
        echo "🔧 Fixing → $rel_path"
        echo "   بود: $current_namespace"
        echo "   شد: $expected_namespace"

        # جایگزینی دقیق فقط خط namespace (حتی اگه ; در انتها نباشه یا { داشته باشه)
        # این sed روی macOS و Linux هر دو کار میکنه
        if sed -i.bak \
            -E "s|^\s*namespace[[:space:]]+[^[:space:];{]+|namespace $expected_namespace|g" \
            "$file" 2>/dev/null; then
            rm -f "$file.bak" 2>/dev/null
        else
            # fallback برای بعضی نسخه‌های قدیمی sed
            cp "$file" "$file.bak"
            sed -E "s|^\s*namespace[[:space:]]+[^[:space:];{]+|namespace $expected_namespace|g" \
                "$file.bak" > "$file"
            rm "$file.bak"
        fi
    fi
done

echo
echo "🎉 همه namespace ها بر اساس مسیر فولدر درست شدن bro! Clean AF 🔥"