#!/bin/sh
set -eu

DEFAULT_API_BASE_URL="http://localhost:5166/api"
RUNTIME_FILE="/usr/share/nginx/html/runtime-config.js"

api_base_url="${API_BASE_URL:-$DEFAULT_API_BASE_URL}"

api_base_url="$(printf "%s" "$api_base_url" | sed 's:/*$::')"

cat > "$RUNTIME_FILE" <<EOF
window.__APP_CONFIG__ = {
    ...(window.__APP_CONFIG__ || {}),
    API_BASE_URL: "$api_base_url"
};
EOF

exec "$@"
    