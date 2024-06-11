# How to reproduce
## Can build/run android
- Open editor and project
- On menu, click 'Bug Test' - 'Not Work Build'
- After build, install 'Build/android_il2cpp/build.apk' and run

## Cannot build/run android
- Open editor and project
- On menu, click 'Bug Test' - Not Work' and 'Addressable Clean All'
- On menu, click 'File' - 'Build setting'
- Select platform
- click Build option (triangle right of Build button) and 'Clean Build'
- Install and run


## Expect
- The color of right cube should turn from gray to green
<img width="201" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/34e0a5b8-516d-4093-a0b1-a200da2a625e">
to
<img width="200" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/63ddb73e-b060-48d1-a44b-1cce69478d65">


## Actual
- The color of right cube will turn from gray to magenta (fail to load shader)
<img width="278" alt="image" src="https://github.com/nakwonchoi-5minlab/SBP-ShaderKeywords-Bug-Final/assets/106501566/0056ee0b-369e-4df0-8a3c-a4d357998e86">

  
