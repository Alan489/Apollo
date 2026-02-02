async function initMap() {
  const gmp = document.querySelector("gmp-map");

  function applyOptions() {
    const map = gmp.innerMap;


    map.setOptions({
      disableDefaultUI: true,
      gestureHandling: 'none',
      keyboardShortcuts: false
    });
  }
  if (gmp.innerMap) {
    google.maps.event.addListenerOnce(gmp.innerMap, "idle", applyOptions);

  } else {

    gmp.addEventListener("gmp-ready", applyOptions);
  }


}

window.initMap = initMap;