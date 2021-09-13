<!DOCTYPE html>
<html lang="en-us">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>For The Birds | Nesting</title>
    <link rel="shortcut icon" href="TemplateData/favicon.ico">
    <link rel="stylesheet" href="TemplateData/style.css">
    <script src="TemplateData/UnityProgress.js"></script>
    <script src="Build/56b27af957420c10aef972428bfcf1ed.js"></script>
    <script>
      var unityInstance = UnityLoader.instantiate("unityContainer", "Build/2ac06bb1b362776cccbea575e597fea4.json", {onProgress: UnityProgress});
    </script>
  </head>

<script src='waxjs.js'></script>

<body>
<div id="loading" ><p id="wait">...loading...</p></div>

	<div class="webgl-content">
	  <div id="logindiv" ></div>
      <div id="unityContainer" style="width: 1280px; height: 720px;z-index:111;position:relative;"></div>
	  <div class="footer">
	  		<div class="fullscreen" onclick="unityInstance.SetFullscreen(1)"></div>
	  </div>
	</div>

<script>

	window.addEventListener("load", function () {
	  // do things after the DOM loads fully
	  console.log("DOM is fully loaded");
	});

  const wax = new waxjs.WaxJS('https://wax.greymass.com');
  const interval = setInterval(function() {
	if (currentProgress < 1.0) {
		return;
	} else {
		WaitForUnity();
		clearInterval(interval);
	}
  }, 1500);
  screen = document.getElementById('loading');
  log = document.getElementById('response'); //Gone for now
  login = document.getElementById('logindiv');

  async function WaitForUnity() {
 	  console.log("Game is loaded");
	  //Add login button
	  var button = document.createElement("button");
	  button.id = "login_button";
	  button.innerHTML = "Login";
	  button.onclick = Login;
	  login.appendChild(button);
	  //remove loading screen
	  screen.style.display = "none";
  }

  async function Login() {
    try {
      const userAccount = await wax.login();
	  login.style.display = "none";
      //log.append(wax.pubKeys);
      //log.append("\n");
      //log.append(userAccount);
	  unityInstance.SendMessage("Networker", "SetLogin", userAccount);
    } catch(e) {
      console.log(e.message);
    }
  }
</script>
</body>
</html>