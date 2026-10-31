import * as THREE from "../ThreeJs/three.module.js";
import { createFaceAvatar } from "./face-avatar.js";

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
    } else if (type === "resetspeechanimation") {
      avatar.reset();
    }
  });
  window.chrome.webview.postMessage({ type: "ready", protocolVersion: 1 });
} else {
  status.textContent = "WebView2 message bridge unavailable";
}