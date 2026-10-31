export function createAbstractAvatar(THREE, scene) {
  const group = new THREE.Group();
  group.visible = false;
  scene.add(group);

  const shellMaterial = new THREE.MeshStandardMaterial({
    color: "#263b3b",
    metalness: 0.72,
    roughness: 0.26,
  });
  const lightMaterial = new THREE.MeshStandardMaterial({
    color: "#f2d9a1",
    emissive: "#f2763d",
    emissiveIntensity: 0.08,
    roughness: 0.2,
    metalness: 0.08,
  });

  const shell = new THREE.Mesh(new THREE.SphereGeometry(0.82, 48, 36), shellMaterial);
  shell.scale.set(1, 1, 0.52);
  group.add(shell);

  const innerRing = new THREE.Mesh(
    new THREE.TorusGeometry(0.54, 0.075, 20, 72),
    new THREE.MeshStandardMaterial({ color: "#aec3b9", metalness: 0.86, roughness: 0.2 }),
  );
  innerRing.position.z = 0.38;
  group.add(innerRing);

  const light = new THREE.Mesh(new THREE.SphereGeometry(0.4, 40, 32), lightMaterial);
  light.position.z = 0.4;
  group.add(light);

  const pointLight = new THREE.PointLight("#ff7544", 0.1, 3.2);
  pointLight.position.set(0, 0, 0.85);
  group.add(pointLight);

  let targetLevel = 0;
  let displayedLevel = 0;
  let pulseIntensity = 1;

  function setAudioLevel(level) {
    targetLevel = Number.isFinite(level) ? Math.min(1, Math.max(0, level)) : 0;
  }

  function setAppearance(appearance) {
    if (typeof appearance.lightColor === "string") {
      lightMaterial.color.set(appearance.lightColor);
      lightMaterial.emissive.set(appearance.lightColor);
      pointLight.color.set(appearance.lightColor);
    }
    if (Number.isFinite(appearance.pulseIntensity)) {
      pulseIntensity = THREE.MathUtils.clamp(appearance.pulseIntensity, 0, 1);
    }
  }

  function update() {
    displayedLevel += (targetLevel - displayedLevel) * 0.28;
    const glow = 0.08 + displayedLevel * 2.4 * pulseIntensity;
    lightMaterial.emissiveIntensity = glow;
    pointLight.intensity = 0.1 + displayedLevel * 4.2 * pulseIntensity;
    const scale = 1 + displayedLevel * 0.1;
    light.scale.setScalar(scale);
  }

  function reset() {
    targetLevel = 0;
  }

  update();
  return { group, setAppearance, setAudioLevel, update, reset };
}