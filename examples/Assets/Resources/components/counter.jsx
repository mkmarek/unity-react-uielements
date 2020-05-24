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

    if (initializing) return <element />;

    const count = counterComponents[0].count;

    return (
        <element style={Object.assign(margin('auto'), { width: "100%", height: "100%", alignItems: "center" })}>
            <element style={countStyle}>{count}</element>
            <element style={{ flexDirection: 'row' }}>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onClick={() => setCounterComponent(0, { count: count + 1 })}>Increment</Button>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onClick={() => setCounterComponent(0, {count: count - 1 })}>Decrement</Button>
            </element>
        </element>
    )
}