export function createImageBasedAvatar(THREE, scene) {
  const group = new THREE.Group();
  group.visible = false;
  scene.add(group);

  const darkMetal = new THREE.MeshStandardMaterial({ color: "#151e23", metalness: 0.78, roughness: 0.28 });
  const rimMaterial = new THREE.MeshStandardMaterial({ color: "#93a69f", metalness: 0.82, roughness: 0.2 });
  const innerMaterial = new THREE.MeshStandardMaterial({ color: "#293b3a", metalness: 0.68, roughness: 0.3 });
  const lensMaterial = new THREE.MeshStandardMaterial({
    color: "#315c58",
    map: createLensTexture(THREE),
    emissive: "#126b5c",
    emissiveIntensity: 0.55,
    metalness: 0.28,
    roughness: 0.16,
  });
  const pupilMaterial = new THREE.MeshPhysicalMaterial({
    color: "#e2a56f",
    emissive: "#d85d27",
    emissiveIntensity: 0.7,
    roughness: 0.14,
    clearcoat: 1,
  });

  const panelShape = new THREE.Shape();
  const width = 1.65;
  const height = 1.35;
  const radius = 0.2;
  panelShape.moveTo(-width / 2 + radius, -height / 2);
  panelShape.lineTo(width / 2 - radius, -height / 2);
  panelShape.quadraticCurveTo(width / 2, -height / 2, width / 2, -height / 2 + radius);
  panelShape.lineTo(width / 2, height / 2 - radius);
  panelShape.quadraticCurveTo(width / 2, height / 2, width / 2 - radius, height / 2);
  panelShape.lineTo(-width / 2 + radius, height / 2);
  panelShape.quadraticCurveTo(-width / 2, height / 2, -width / 2, height / 2 - radius);
  panelShape.lineTo(-width / 2, -height / 2 + radius);
  panelShape.quadraticCurveTo(-width / 2, -height / 2, -width / 2 + radius, -height / 2);

  const panel = new THREE.Mesh(
    new THREE.ExtrudeGeometry(panelShape, { depth: 0.14, bevelEnabled: true, bevelThickness: 0.04, bevelSize: 0.04, bevelSegments: 3 }),
    darkMetal,
  );
  group.add(panel);

  const innerPlate = new THREE.Mesh(new THREE.BoxGeometry(1.42, 1.12, 0.08), innerMaterial);
  innerPlate.position.z = 0.17;
  group.add(innerPlate);

  for (const x of [-0.62, 0.62]) {
    const fastener = new THREE.Mesh(new THREE.CylinderGeometry(0.035, 0.035, 0.025, 16), rimMaterial);
    fastener.rotation.x = Math.PI / 2;
    fastener.position.set(x, 0.46, 0.23);
    group.add(fastener);
  }

  const lensBacking = new THREE.Mesh(new THREE.TorusGeometry(0.47, 0.095, 16, 64), rimMaterial);
  lensBacking.position.set(0, 0.02, 0.24);
  group.add(lensBacking);

  const innerRing = new THREE.Mesh(new THREE.TorusGeometry(0.355, 0.035, 12, 64), darkMetal);
  innerRing.position.set(0, 0.02, 0.31);
  group.add(innerRing);

  const lens = new THREE.Mesh(new THREE.SphereGeometry(1, 40, 32), lensMaterial);
  lens.scale.set(0.335, 0.335, 0.085);
  lens.position.set(0, 0.02, 0.3);
  group.add(lens);

  const pupil = new THREE.Mesh(new THREE.SphereGeometry(1, 32, 24), pupilMaterial);
  pupil.scale.set(0.105, 0.105, 0.055);
  pupil.position.set(0, 0.02, 0.38);
  group.add(pupil);

  const lightBars = [];
  for (let index = 0; index < 12; index++) {
    const angle = (index / 12) * Math.PI * 2;
    const bar = new THREE.Mesh(new THREE.BoxGeometry(0.035, 0.11, 0.025), rimMaterial);
    bar.position.set(Math.cos(angle) * 0.62, 0.02 + Math.sin(angle) * 0.62, 0.22);
    bar.rotation.z = -angle;
    group.add(bar);
    lightBars.push(bar);
  }

  let speechEnergy = 0;
  function setViseme(visemeId) {
    speechEnergy = visemeId === 0 ? 0 : [1, 2, 3, 8, 9, 10, 11].includes(visemeId) ? 1 : 0.55;
  }

  function update(timeSeconds) {
    group.rotation.y = Math.sin(timeSeconds * 0.24) * 0.055;
    lensMaterial.emissiveIntensity = 0.5 + speechEnergy * 1.6;
    pupilMaterial.emissiveIntensity = 0.55 + speechEnergy * 1.8;
    for (const [index, bar] of lightBars.entries()) {
      const active = speechEnergy > 0 && Math.sin(timeSeconds * 9 + index) > 0.2;
      bar.material = active ? pupilMaterial : rimMaterial;
    }
  }

  function reset() {
    speechEnergy = 0;
  }

  return { group, setViseme, update, reset };
}

function createLensTexture(THREE) {
  const canvas = document.createElement("canvas");
  canvas.width = 256;
  canvas.height = 256;
  const context = canvas.getContext("2d");
  if (!context) throw new Error("Could not create the local lens texture.");

  const gradient = context.createRadialGradient(128, 128, 4, 128, 128, 124);
  gradient.addColorStop(0, "#d8a57e");
  gradient.addColorStop(0.18, "#764f42");
  gradient.addColorStop(0.42, "#237f70");
  gradient.addColorStop(0.72, "#152d32");
  gradient.addColorStop(1, "#0b151a");
  context.fillStyle = gradient;
  context.fillRect(0, 0, canvas.width, canvas.height);

  context.strokeStyle = "rgba(157, 223, 204, 0.35)";
  context.lineWidth = 2;
  for (let index = 0; index < 24; index++) {
    const angle = (index / 24) * Math.PI * 2;
    context.beginPath();
    context.moveTo(128 + Math.cos(angle) * 62, 128 + Math.sin(angle) * 62);
    context.lineTo(128 + Math.cos(angle) * 116, 128 + Math.sin(angle) * 116);
    context.stroke();
  }

  const texture = new THREE.CanvasTexture(canvas);
  texture.colorSpace = THREE.SRGBColorSpace;
  return texture;
}