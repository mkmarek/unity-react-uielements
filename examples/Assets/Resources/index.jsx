import React, { useState } from 'react';
import { render } from 'unity-renderer';
import TabPanel from './components/tab-panel';
// import Counter from './components/counter';
// import ComponentPreview from './components/component-preview';
// import Todo from './components/Todo';
import Popup from './components/popup';

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

const Tab1Style = Object.assign(margin('auto'), {
  alignSelf: 'center',
  unityTextAlign: 'MiddleCenter'
});

function App() {
  const [popupOpened, setPopupOpened] = useState(false);

  return (
    <visualElement pickingMode="ignore" style={rootStyle}>
      <TabPanel style={tabPanelStyle}>
        <TabPanel.Panel name="Welcome">
          <visualElement style={Tab1Style}>
            A demo of React used inside Unity
            <visualElement style={{ flexDirection: 'row', marginTop: 25 }}>
              <visualElement style={{ width: 256, height: 86, marginRight: 10, backgroundImage: 'images/react-logo' }} />
              <visualElement style={{  width: 256, height: 93, backgroundImage: 'images/unity-logo' }} />
            </visualElement>
          </visualElement>
        </TabPanel.Panel>
        <TabPanel.Panel name="Counter">
          {/* <Counter /> */}
        </TabPanel.Panel>
        <TabPanel.Panel name="Component test">
          {/* <ComponentPreview /> */}
        </TabPanel.Panel>
        <TabPanel.Panel name="ECS TODO example">
          {/* <Todo /> */}
        </TabPanel.Panel>
        <TabPanel.Panel name="Popup example">
          <button onMouseUpEvent={() => setPopupOpened(true)} text="Open popup" />
        </TabPanel.Panel>
      </TabPanel>
      {popupOpened && <Popup onClose={() => setPopupOpened(false)} />}
    </visualElement>
  );
}

render(<App />)

