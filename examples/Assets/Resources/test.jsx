const CounterComponent = getFactory('CounterComponent')
var component = new CounterComponent();

console.log(CounterComponent);
console.log(component);

component.setIntTest(1434);
component.setShortTest(633);
component.setFloatTest(0.55);
component.setDoubleTest(0.321);

console.log(component.getIntTest());
console.log(component.getShortTest());
console.log(component.getFloatTest());
console.log(component.getDoubleTest());

const nested = component.getNested();
nested.setTest(123);
component.setNested(nested);

console.log(component.getNested().getTest());