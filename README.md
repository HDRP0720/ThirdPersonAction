### Version
* Unity 2022.3.15f1

### Used Asset
* [Hot Reload](https://assetstore.unity.com/packages/tools/utilities/hot-reload-edit-code-without-compiling-254358)
* [Unity-Chan! Sunny Side Up(URP)](https://github.com/unity3d-jp/UnityChanSSU/releases/download/1.0.5/UnityChanSSU_URP-release-1.0.5.zip)
  * 셰이더 적용을 위해 `URP-HighFidelity-Renderer`설정의 `Depth Priming Mode`를 `Disabled`로 변경
  * Unity-Chan SSU를 위해선 Packages/manifest.json 파일 내에 다음과 같은 코드를 작성하여 추가해주어야 합니다.
    ```json
    "com.unity.toonshader": "0.9.5-preview",
    "com.unity.springbone": "https://github.com/unity3d-jp/UnityChanSpringBone.git"
    ```