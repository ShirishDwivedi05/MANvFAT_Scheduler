 const dial = document.querySelector('.dial');
    const lbs = document.querySelector('#lbs-value');
    const kg = document.querySelector('#kg-value');
    const hDial = new Hammer(dial, { direction: Hammer.DIRECTION_HORIZONTAL });
    let state = { weight: 125, x: 16 };
    lbs.innerText = state.weight;
    kg.innerText = lbsToKg(state.weight).toFixed(1);

    const dialPan$ = Rx.Observable
      .fromEventPattern(function(e){ return hDial.on('pan panend', e)}, function() {})
      .map(function(obj) {
        if (obj.type === 'panend') {
          state = updateState(state, { delta: obj.deltaX });
          //return `${ state.x }px`;
		  return state.x + "px";
        }
       
        render(state.weight - obj.deltaX * 5 / 75);
        //return `${ state.x + obj.deltaX }px`;
		return (state.x + obj.deltaX) + "px";
      });

const style$ = RxCSS({
  x: dialPan$,
});

function render(weight) {
  lbs.innerText = weight.toFixed(0);
  kg.innerText = lbsToKg(weight).toFixed(1);
}

function updateState(prevState, obj) {
  prevState = prevState || 0;
  const snappedDelta = (Math.round(obj.delta / 15) * 15);
  
  return {
    weight: prevState.weight - (snappedDelta * 5 / 75),
    x: prevState.x + snappedDelta,
  };
}

function lbsToKg(weight) {
  return weight * 0.453592;
}

//IE 11 hack for updating background position for dial
let isDialMove = false;
if(!!window.MSInputMethodContext && !!document.documentMode) {
	//fixed. it's breaking the boundry of parent container.
	dial.parentElement.style.width = '100%';
	dial.addEventListener('mousedown', function(e){isDialMove = true});
	dial.addEventListener('mouseup', function(e){isDialMove = false});
	dial.addEventListener('mouseup', function(e){isDialMove = false});
	document.body.addEventListener('mouseup', function(e){isDialMove = false});	
	dial.addEventListener('mousemove', function(e){
		if(!isDialMove) {
			return;
		}
		this.style.backgroundPositionX = e.offsetX + "px";
	});
}