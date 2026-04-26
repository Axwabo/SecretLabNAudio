# must be executed in the build output directory

mkdir SecretLabNAudio
mkdir SecretLabNAudio/bin
mkdir SecretLabNAudio/LICENSES

mv SecretLabNAudio.dll SecretLabNAudio/bin
ls | grep -E "^(NAudio|NLayer|NVorbis|System.ValueTuple)" | xargs -I {0} mv {0} SecretLabNAudio/bin

cp -r ../THIRD_PARTY_LICENSES SecretLabNAudio/LICENSES
cp ../LICENSE SecretLabNAudio/LICENSES/SecretLabNAudio.txt

cd SecretLabNAudio
zip -r ../SecretLabNAudio.zip .
