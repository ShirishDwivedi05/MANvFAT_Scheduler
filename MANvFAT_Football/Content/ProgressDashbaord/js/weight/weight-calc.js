 const dial = document.querySelector('.dial');
    const lbs = document.querySelector('#lbs-value');
    const kg = document.querySelector('#kg-value');
    const hDial = new Hammer(dial, { direction: Hammer.DIRECTION_HORIZONTAL });
    let state = { weight: 125, x: 16 };
    lbs.innerText = state.weight;
    kg.innerText = lbsToKg(state.weight).toFixed(1);

    const dialPan$ = Rx.Observable
      .fromEventPattern(e => hDial.on('pan panend', e), () => {})
      .map(({ deltaX, type, direction }) => {
        if (type === 'panend') {
          state = updateState(state, { delta: deltaX });
          return `${ state.x }px`;
        }
       
        render(state.weight - deltaX * 5 / 75);
        return `${ state.x + deltaX }px`;
      });

const style$ = RxCSS({
  x: dialPan$,
});

function render(weight) {
  lbs.innerText = weight.toFixed(0);
  kg.innerText = lbsToKg(weight).toFixed(1);
}

function updateState(prevState = 0, { delta }) {
  const snappedDelta = (Math.round(delta / 15) * 15);
  
  return { 
    weight: prevState.weight - (snappedDelta * 5 / 75),
    x: prevState.x + snappedDelta,
  };
}

function lbsToKg(weight) {
  return weight * 0.453592;
}