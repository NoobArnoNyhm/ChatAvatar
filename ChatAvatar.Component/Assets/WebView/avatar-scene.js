import * as THREE from "../ThreeJs/three.module.js";

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

const material = new THREE.MeshStandardMaterial({
  color: "#22b8a7",
  metalness: 0.24,
  roughness: 0.34,
});
const avatar = new THREE.Mesh(new THREE.IcosahedronGeometry(0.9, 1), material);
scene.add(avatar);

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
  avatar.rotation.y = seconds * 0.32;
  avatar.rotation.x = Math.sin(seconds * 0.45) * 0.12;
  renderer.render(scene, camera);
});

if (window.chrome?.webview) {
  window.chrome.webview.addEventListener("message", ({ data }) => {
    if (data?.protocolVersion !== 1) return;
    if (String(data.type).toLowerCase() === "initialize") {
      status.textContent = "Three.js scene initialized";
    }
  });
  window.chrome.webview.postMessage({ type: "ready", protocolVersion: 1 });
} else {
  status.textContent = "WebView2 message bridge unavailable";
}