import React from 'react';
import { render } from 'unity-renderer';
import TabPanel from './components/tab-panel';
import Counter from './components/counter';
import ComponentPreview from './components/component-preview';
import Todo from './components/Todo';

const margin = (margin) => ({
  marginTop: margin,
  marginBottom: margin,
  marginLeft: margin,
  marginRight: margin
});

const rootStyle = {
  width: "100%",
  height: "100%",
  flexDirection: 'row'
};

const tabPanelStyle = Object.assign(
  margin('auto'),
  { width: "80%", height: "80%", alignSelf: 'center' });

const Tab1Style = Object.assign(margin('auto'), { alignSelf: 'center' });

function App() {
  return (
    <element style={rootStyle}>
      <TabPanel style={tabPanelStyle}>
        <TabPanel.Panel name="Welcome">
          <element style={Tab1Style}>
            A demo of React used inside Unity
          </element>
        </TabPanel.Panel>
        <TabPanel.Panel name="Counter">
          <Counter />
        </TabPanel.Panel>
        <TabPanel.Panel name="Component test">
          <ComponentPreview />
        </TabPanel.Panel>
        <TabPanel.Panel name="ECS TODO example">
          <Todo />
        </TabPanel.Panel>
      </TabPanel>
    </element>
  );
}

render(<App />)

