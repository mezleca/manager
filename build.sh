#!/bin/bash

PROJECT_ROOT=$(pwd)
BUILD_DIR="$PROJECT_ROOT/build"
OUTPUT_DIR="$PROJECT_ROOT/dist"

# get project name
CSPROJ_FILE=$(find . -maxdepth 1 -name "*.csproj" | head -1)
if [ -z "$CSPROJ_FILE" ]; then
    error "no .csproj file found"
fi

APP_NAME=$(basename "$CSPROJ_FILE" .csproj)
APP_VERSION="2.0.0"

# colors
RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m'

log() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
    exit 1
}

check_deps() {
    log "checking dependencies..."
    
    if ! command -v dotnet &> /dev/null; then
        error "dotnet not found"
    fi
    
    if ! command -v zip &> /dev/null; then
        error "zip not found"
    fi
    
    # check for appimage tools
    if ! command -v appimagetool &> /dev/null; then
        log "installing appimage tools..."
        wget -O appimagetool https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage
        chmod +x appimagetool
        sudo mv appimagetool /usr/local/bin/
    fi
}

build_project() {
    local platform=$1
    log "building for $platform..."
    
    # clean everything
    rm -rf obj bin

    dotnet clean "$CSPROJ_FILE"
    dotnet restore "$CSPROJ_FILE" -r $platform
    dotnet publish "$CSPROJ_FILE" -r $platform -c Release --self-contained true -p:PublishSingleFile=true -p:Version=$APP_VERSION -o "$BUILD_DIR/$platform" || error "build failed for $platform"
}

setup_dirs() {
    log "setting up directories..."
    rm -rf "$BUILD_DIR" "$OUTPUT_DIR"
    mkdir -p "$BUILD_DIR" "$OUTPUT_DIR"
}

create_windows_package() {
    log "creating windows package..."
    cd "$BUILD_DIR/win-x64"
    zip -r "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-win-x64.zip" . || error "failed to create windows zip"
    cd "$PROJECT_ROOT"
}

create_deb_package() {
    log "creating deb package..."
    
    local deb_dir="$BUILD_DIR/deb"

    mkdir -p "$deb_dir/DEBIAN"
    mkdir -p "$deb_dir/usr/bin"
    mkdir -p "$deb_dir/usr/share/applications"
    mkdir -p "$deb_dir/usr/share/icons/hicolor/512x512/apps"
    
    # find executable
    local exe_path=$(find "$BUILD_DIR/linux-x64" -type f -executable | head -1)
    if [ -z "$exe_path" ]; then
        error "failed to get linux binary"
    fi
    
    local exe_name=$(basename "$exe_path")
    
    # copy binary
    cp "$exe_path" "$deb_dir/usr/bin/" || error "failed to copy binary"
    chmod +x "$deb_dir/usr/bin/$exe_name"
    
    # copy icon if exists
    if [ -f "frontend/static/icon.png" ]; then
        cp "frontend/static/icon.png" "$deb_dir/usr/share/icons/hicolor/512x512/apps/$APP_NAME.png"
    fi
    
    # create control file
    cat > "$deb_dir/DEBIAN/control" << EOF
Package: $(echo $APP_NAME | tr '[:upper:]' '[:lower:]')
Version: $APP_VERSION
Section: utils
Priority: optional
Architecture: amd64
Maintainer: Developer <yes@yes.com>
Description: $APP_NAME application
EOF

    # create desktop entry
    cat > "$deb_dir/usr/share/applications/$APP_NAME.desktop" << EOF
[Desktop Entry]
Name=$APP_NAME
Comment=$APP_NAME application
Exec=/usr/bin/$exe_name
Icon=$APP_NAME
Terminal=false
Type=Application
Categories=Utility;
EOF
    
    # build deb package
    dpkg-deb --build "$deb_dir" "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-linux-x64.deb" || error "failed to create deb package"
}

create_appimage() {
    log "creating appimage..."
    
    local appdir="$BUILD_DIR/AppDir"

    mkdir -p "$appdir/usr/bin"
    mkdir -p "$appdir/usr/share/applications"
    mkdir -p "$appdir/usr/share/icons/hicolor/512x512/apps"
    
    # find the actual executable
    local exe_path=$(find "$BUILD_DIR/linux-x64" -type f -executable | head -1)
    if [ -z "$exe_path" ]; then
        error "no executable found in linux build"
    fi
    
    local exe_name=$(basename "$exe_path")
    
    # copy binary
    cp "$exe_path" "$appdir/usr/bin/" || error "failed to copy binary for appimage"
    chmod +x "$appdir/usr/bin/$exe_name"
    
    # copy icon ()
    if [ -f "frontend/static/icon.png" ]; then
        cp "frontend/static/icon.png" "$appdir/$APP_NAME.png"
        cp "frontend/static/icon.png" "$appdir/usr/share/icons/hicolor/512x512/apps/$APP_NAME.png"
    fi
    
    # create desktop entry
    cat > "$appdir/$APP_NAME.desktop" << EOF
[Desktop Entry]
Name=$APP_NAME
Comment=$APP_NAME application
Exec=$exe_name
Icon=$APP_NAME
Terminal=false
Type=Application
Categories=Utility;
EOF
    
    cp "$appdir/$APP_NAME.desktop" "$appdir/usr/share/applications/"
    
    # create apprun
    cat > "$appdir/AppRun" << EOF
#!/bin/bash
SELF=\$(readlink -f "\$0")
HERE=\${SELF%/*}
export PATH="\${HERE}/usr/bin/:\${PATH}"
exec "\${HERE}/usr/bin/$exe_name" "\$@"
EOF
    chmod +x "$appdir/AppRun"
    
    # build appimage
    cd "$BUILD_DIR"
    ARCH=x86_64 appimagetool AppDir "$OUTPUT_DIR/$APP_NAME-$APP_VERSION-linux-x64.AppImage" || error "failed to create appimage"
    cd "$PROJECT_ROOT"
}

main() {
    log "starting build process for $APP_NAME v$APP_VERSION"
    
    check_deps
    setup_dirs

    # build for windows
    build_project win-x64
    create_windows_package
    
    # build for linux
    build_project linux-x64
    create_deb_package
    create_appimage
    
    ls -la "$OUTPUT_DIR"
}

main "$@"