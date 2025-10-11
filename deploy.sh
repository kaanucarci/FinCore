#!/bin/bash

# === FinCore Deploy Script ===
# Kaan Uçarcı — Azure App Service otomatik yayın

# CONFIG
RESOURCE_GROUP="düşük"
APP_NAME="fincore"
PROJECT_PATH="./FinCore.Api/FinCore.Api.csproj"
PUBLISH_DIR="./publish"
ZIP_FILE="./FinCore.Api.zip"

# 1️⃣ Build temizliği
echo "🧹 Eski build temizleniyor..."
dotnet clean

# 2️⃣ Release modda publish et
echo "⚙️ API publish ediliyor..."
dotnet publish $PROJECT_PATH -c Release -o $PUBLISH_DIR

# 3️⃣ Eski zip varsa sil
if [ -f "$ZIP_FILE" ]; then
  echo "🗑️  Eski zip dosyası siliniyor..."
  rm $ZIP_FILE
fi

# 4️⃣ Zip oluştur
echo "📦 Yeni zip oluşturuluyor..."
cd $PUBLISH_DIR
zip -r ../FinCore.Api.zip .
cd ..

# 5️⃣ Azure’a gönder
echo "☁️  Azure’a deploy başlatılıyor..."
az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $APP_NAME \
  --src $ZIP_FILE

# 6️⃣ Deploy tamamlandıktan sonra zip'i sil
if [ -f "$ZIP_FILE" ]; then
  echo "🧽 Deploy tamamlandı. Zip dosyası siliniyor..."
  rm "$ZIP_FILE"
fi

echo "✅ Deploy işlemi başarıyla tamamlandı!"
