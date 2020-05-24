import React, { useState } from 'react';
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

export default function Counter() {
    const [count, setCount] = useState(0);

    return (
        <visualElement style={{
            width: "100%",
            height: "100%",
            alignItems: 'Center',
            justifyContent: 'Center'}}>
            <visualElement style={countStyle}>{count}</visualElement>
            <visualElement style={{ flexDirection: 'Row' }}>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCount(count + 1)}>Increment</Button>
                <Button
                    style={Object.assign(itemStyle, { fontSize: '18'})}
                    onMouseUpEvent={() => setCount(count - 1)}>Decrement</Button>
            </visualElement>
        </visualElement>
    )
}