import * as THREE from "../ThreeJs/three.module.js";
import { createFaceAvatar } from "./face-avatar.js";
import { createImageBasedAvatar } from "./image-based-avatar.js";
import { createAbstractAvatar } from "./abstract-avatar.js";

const canvas = document.querySelector("#avatar-scene");
const status = document.querySelector("#status");
const scene = new THREE.Scene();
scene.background = new THREE.Color("#101820");

const camera = new THREE.PerspectiveCamera(42, 1, 0.1, 100);
camera.position.set(0, 0, 4);

const renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
renderer.setPixelRatio(Math.min(window.devicePixelRatio || 1, 2));

scene.add(new THREE.HemisphereLight("#d9f4f3", "#14212a", 2.2));
const keyLight = new THREE.DirectionalLight("#ffffff", 3);
keyLight.position.set(2, 3, 4);
scene.add(keyLight);

const avatar = createFaceAvatar(THREE, scene);
const imageBasedAvatar = createImageBasedAvatar(THREE, scene);
const abstractAvatar = createAbstractAvatar(THREE, scene);

function resizeScene() {
  const width = canvas.clientWidth;
  const height = canvas.clientHeight;
  if (width === 0 || height === 0) return;

  camera.aspect = width / height;
  camera.updateProjectionMatrix();
  renderer.setSize(width, height, false);
}

new ResizeObserver(resizeScene).observe(canvas);
resizeScene();

renderer.setAnimationLoop((time) => {
  const seconds = time * 0.001;
  avatar.face.rotation.y = Math.sin(seconds * 0.28) * 0.08;
  avatar.face.rotation.x = Math.sin(seconds * 0.45) * 0.025;
  imageBasedAvatar.update(seconds);
  abstractAvatar.update();
  renderer.render(scene, camera);
});

if (window.chrome?.webview) {
  window.chrome.webview.addEventListener("message", ({ data }) => {
    if (data?.protocolVersion !== 1) return;
    const type = String(data.type).toLowerCase();
    if (type === "initialize") {
      status.textContent = "3D face avatar ready";
    } else if (type === "setviseme" && Number.isInteger(data.visemeId)) {
      avatar.setViseme(data.visemeId);
      imageBasedAvatar.setViseme(data.visemeId);
    } else if (type === "resetspeechanimation") {
      avatar.reset();
      imageBasedAvatar.reset();
      abstractAvatar.reset();
    } else if (type === "setavatartype" && typeof data.avatarType === "string") {
      const avatarType = data.avatarType.toLowerCase();
      avatar.face.visible = avatarType === "face";
      imageBasedAvatar.group.visible = avatarType === "imagebased";
      abstractAvatar.group.visible = avatarType === "abstract";
    } else if (type === "setaudiolevel" && Number.isFinite(data.audioLevel)) {
      abstractAvatar.setAudioLevel(data.audioLevel);
    } else if (type === "setavatarappearance" && typeof data.avatarType === "string") {
      const avatarType = data.avatarType.toLowerCase();
      avatar.face.visible = avatarType === "face";
      imageBasedAvatar.group.visible = avatarType === "imagebased";
      abstractAvatar.group.visible = avatarType === "abstract";
      if (avatarType === "face") avatar.setAppearance(data);
      if (avatarType === "imagebased") imageBasedAvatar.setAppearance(data);
      if (avatarType === "abstract") abstractAvatar.setAppearance(data);
    }
  });
  window.chrome.webview.postMessage({ type: "ready", protocolVersion: 1 });
} else {
  status.textContent = "WebView2 message bridge unavailable";
}