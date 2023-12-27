### Version
* Unity 2022.3.15f1  

### Used Asset
* [Amplify Animation Pack](https://assetstore.unity.com/packages/3d/animations/amplify-animation-pack-207911)
* [Long Sword](https://assetstore.unity.com/packages/3d/props/weapons/long-sword-212082)
* [Polygon Dungeon](https://assetstore.unity.com/packages/3d/environments/dungeons/polygon-dungeons-low-poly-3d-art-by-synty-102677)
* [Unity-Chan! Sunny Side Up(URP)](https://github.com/unity3d-jp/UnityChanSSU/releases/download/1.0.5/UnityChanSSU_URP-release-1.0.5.zip)
  * 전용 셰이더 적용을 위해 `URP-HighFidelity-Renderer`설정의 `Depth Priming Mode`를 `Disabled`로 변경
  * Unity-Chan SSU를 위해서 `Packages/manifest.json` 파일 내에 아래 코드를 작성하여 추가.
    ```json
    "com.unity.toonshader": "0.9.5-preview",
    "com.unity.springbone": "https://github.com/unity3d-jp/UnityChanSpringBone.git"
    ```
* Mixamo  
캐릭터 모델 파일을 업로드 (애니메이션 파일의 본구조&이름을 캐릭터에 자동으로 맞춰 주는 작업이 믹사모에서 자동으로 이루어진다.)
  * For Parkour
    * [Male Jumping Onto Platform](https://www.mixamo.com/#/?page=1&query=Male+Jumping+Onto+Platform&type=Motion%2CMotionPack)
    * [Fast Run To Jump Up Onto Higher Level](https://www.mixamo.com/#/?page=1&query=Fast+Run+To+Jump+Up+Onto+Higher+Level&type=Motion%2CMotionPack)
    * [Sprint To Wall Climb To Crouch Idle](https://www.mixamo.com/#/?page=1&query=Sprint+To+Wall+Climb+To+Crouch+Idle&type=Motion%2CMotionPack)
    * [Crouched To Standing](https://www.mixamo.com/#/?page=1&query=Crouched+To+Standing&type=Motion%2CMotionPack)
  * For Fight
    * [Brutal Assassination animation set](https://www.mixamo.com/#/?page=1&query=brutal+assassination&type=Motion%2CMotionPack)  
  
### Reference
* [Quick Unity Tips: From Root Motion to In Place Animations](https://youtu.be/SGboqxemhok?si=S3EhWE_A53Ea9cLt)