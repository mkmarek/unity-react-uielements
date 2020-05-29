import React from 'react';
import { useQuery } from 'unity-renderer';
import Button from './button';

const margin = (margin) => ({
    marginTop: margin,
    marginBottom: margin,
    marginLeft: margin,
    marginRight: margin
});

const itemStyle = Object.assign(margin('5px'), {
    width: "100px",
    height: "100px",
    color: "#ffffff"
});

const countStyle = Object.assign(margin('5px'), {
    marginTop: "25px",
    fontSize: '24',
    alignItems: "center",
    alignContent: "center",
    justifyContent: "center",
    flexDirection: "column",
    color: "#ffffff",
    height: '40%'
});

const counterComponentName = 'CounterComponent';
const CounterComponent = getFactory(counterComponentName);

export default function Counter() {
    const query = useQuery([counterComponentName]);

    if (query.getSize() == 0) return <></>;
    
    const { Count } = query.getElementAt(counterComponentName, 0);

    const setCount = (value) => {
        const counterComponentEntity = query.getElementAt('Entity', 0);

        const newValue = new CounterComponent();
        newValue.Count = value;

        executeBuffer((buffer) => {
            buffer.setComponent(counterComponentName, counterComponentEntity, newValue);
        })
    }

    return (
        <visualElement style={{
            width: "100%",
            height: "100%",
            alignItems: 'Center',
            justifyContent: 'Center'}}>
            <visualElement style={countStyle}>{Count}</visualElement>
            <visualElement style={{ flexDirection: 'Row' }}>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCount(Count + 1)}>Increment</Button>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCount(Count - 1)}>Decrement</Button>
            </visualElement>
        </visualElement>
    )
}