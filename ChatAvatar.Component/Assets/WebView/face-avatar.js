export function createFaceAvatar(THREE, scene) {
  const face = new THREE.Group();
  scene.add(face);

  const skin = new THREE.MeshStandardMaterial({ color: "#c98e70", roughness: 0.62 });
  const shadow = new THREE.MeshStandardMaterial({ color: "#51372f", roughness: 0.82 });
  const eyeWhite = new THREE.MeshStandardMaterial({ color: "#f4eee3", roughness: 0.3 });
  const iris = new THREE.MeshStandardMaterial({ color: "#326f70", roughness: 0.24 });
  const pupil = new THREE.MeshStandardMaterial({ color: "#14212a", roughness: 0.18 });

  const head = new THREE.Mesh(new THREE.SphereGeometry(1, 48, 40), skin);
  head.scale.set(0.76, 1.02, 0.58);
  face.add(head);

  for (const side of [-1, 1]) {
    const ear = new THREE.Mesh(new THREE.SphereGeometry(0.16, 24, 20), skin);
    ear.scale.set(0.48, 0.92, 0.45);
    ear.position.set(side * 0.73, -0.02, 0.02);
    face.add(ear);

    const eye = new THREE.Mesh(new THREE.SphereGeometry(1, 32, 24), eyeWhite);
    eye.scale.set(0.145, 0.095, 0.055);
    eye.position.set(side * 0.29, 0.24, 0.49);
    face.add(eye);

    const irisMesh = new THREE.Mesh(new THREE.SphereGeometry(1, 24, 20), iris);
    irisMesh.scale.set(0.052, 0.062, 0.028);
    irisMesh.position.set(side * 0.29, 0.235, 0.54);
    face.add(irisMesh);

    const pupilMesh = new THREE.Mesh(new THREE.SphereGeometry(1, 20, 16), pupil);
    pupilMesh.scale.set(0.024, 0.038, 0.015);
    pupilMesh.position.set(side * 0.29, 0.235, 0.562);
    face.add(pupilMesh);

    const brow = new THREE.Mesh(new THREE.BoxGeometry(0.28, 0.045, 0.06), shadow);
    brow.position.set(side * 0.29, 0.39, 0.51);
    brow.rotation.z = side * -0.08;
    face.add(brow);
  }

  const nose = new THREE.Mesh(new THREE.SphereGeometry(1, 24, 20), skin);
  nose.scale.set(0.09, 0.17, 0.15);
  nose.position.set(0, 0.015, 0.57);
  face.add(nose);

  const mouth = new THREE.Mesh(new THREE.SphereGeometry(1, 32, 20), shadow);
  mouth.position.set(0, -0.38, 0.52);
  face.add(mouth);

  function setViseme(visemeId) {
    const mouthOpening = visemeId === 0 ? 0.015
      : [1, 2, 3, 8, 9, 10, 11].includes(visemeId) ? 0.13
        : [4, 5, 6, 7].includes(visemeId) ? 0.085
          : [19, 20, 21].includes(visemeId) ? 0.025
            : 0.05;
    const mouthWidth = [1, 2, 3, 8, 9, 10, 11].includes(visemeId) ? 0.2 : 0.16;
    mouth.scale.set(mouthWidth, mouthOpening, 0.035);
  }

  function reset() {
    setViseme(0);
  }

  reset();
  return { face, setViseme, reset };
}