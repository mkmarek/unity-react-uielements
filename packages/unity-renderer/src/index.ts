import reconciler from 'react-reconciler/persistent';
import Element from './components/element';
import ContainerElement from './components/container-element';

import * as base from './host-config/base'
import * as mutation from './host-config/mutation'
import * as persistence from './host-config/persistence'
import Bridge from './native-to-js-bridge';

const renderer = reconciler({
    isPrimaryRenderer: true,

    ...base,
    ...mutation,
    ...persistence
});

let containerElement = null;
let container = null;

export function render(element: React.ReactNode, callback: () => void = () => {}) {

    if (!containerElement || !container) {
        containerElement = new ContainerElement('element', {});
        container = renderer.createContainer(containerElement, false, false);
    }

    global.bridge = new Bridge(containerElement);
    global.natives.nativeToJsBridge = global.bridge.onMessage.bind(global.bridge);

    renderer.updateContainer(element, container, null, callback);
}

export * from './hooks';
export * from './actions';