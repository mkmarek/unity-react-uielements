import React, { useState } from 'react';
import Panel from './panel';
import Button from './button';

export default function TabPanel({ style, children }) {
    const [tabIndex, setTabIndex] = useState(0);

    return (
        <element style={Object.assign(style, { color: '#ffffff', fontSize: '18px' })}>
            <element style={{ flexDirection: 'row', width: '100%', height: '64px' }}>
            {children.map((e, i) =>
                <Button
                    key={i}
                    onClick={() => setTabIndex(i)}
                    innerStyle={{ backgroundColor: i === tabIndex ? '#003f6b' : '#133b50' }}
                    style={{ width: '128px', height: '100%' }}>
                    {e.props.name}
                </Button>)}
            </element>
            <Panel outerStyle={{}} innerStyle={{}}>
                {children[tabIndex].props.children}
            </Panel>
        </element>
    )
}

TabPanel.Panel = function({ name, children }) {
    return null;
}