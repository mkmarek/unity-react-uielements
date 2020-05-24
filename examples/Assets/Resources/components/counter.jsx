import React, { useState } from 'react';
import Button from './button';
import { useQuery } from 'unity-renderer';

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

export default function Counter() {
    const [initializing, counterComponents, setCounterComponent] = useQuery('Counter');

    if (initializing) return <visualElement />;

    const count = counterComponents[0].count;

    return (
        <visualElement style={Object.assign(margin('auto'), { width: "100%", height: "100%", alignItems: "center" })}>
            <visualElement style={countStyle}>{count}</visualElement>
            <visualElement style={{ flexDirection: 'row' }}>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCounterComponent(0, { count: count + 1 })}>Increment</Button>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCounterComponent(0, {count: count - 1 })}>Decrement</Button>
            </visualElement>
        </visualElement>
    )
}