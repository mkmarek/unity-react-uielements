import React from 'react';
import { render } from 'unity-renderer'

const App = () => (
    <visualElement style={{ width: '100%', height: '100%', backgroundColor: '#ffffff', color: '#000000' }}>
        <visualElement style={{ width: '50%', height: '50%', backgroundColor: '#ff0000', color: '#000000' }}>
            fdsklfskfsd
            <textElement style={{ width: '100%', height: '100%',color: '#000000', fontSize: 20 }} text="Some stuff" />
        </visualElement>
    </visualElement>
)

render(<App />);

// const CounterComponent = getFactory('CounterComponent')
// var component = new CounterComponent();

// console.log(CounterComponent);
// console.log(component);

// component.setIntTest(1434);
// component.setShortTest(633);
// component.setFloatTest(0.55);
// component.setDoubleTest(0.321);

// console.log(component.getIntTest());
// console.log(component.getShortTest());
// console.log(component.getFloatTest());
// console.log(component.getDoubleTest());

// const nested = component.getNested();
// nested.setTest(123);
// component.setNested(nested);

// console.log(component.getNested().getTest());