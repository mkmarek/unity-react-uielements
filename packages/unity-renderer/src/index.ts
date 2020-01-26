import reconciler from 'react-reconciler/persistent';
import Element from './components/element';
import ContainerElement from './components/container-element';

import * as base from './host-config/base'
import * as mutation from './host-config/mutation'
import * as persistence from './host-config/persistence'

const renderer = reconciler({
    isPrimaryRenderer: true,

    ...base,
    ...mutation,
    ...persistence
});

export function render(element: React.ReactNode, callback: () => void = () => {}) {

    const containerElement = new ContainerElement('element', {});
    const container = renderer.createContainer(containerElement, false, false);

    global.natives.invokeCallback = (id, callbackName) => {
        let queue: Element[] = [containerElement];

        while (queue.length > 0) {
            const item = queue.pop();

            if (item.id === id) {
                item.props[callbackName] && item.props[callbackName]();
                return true;
            }

            for (let child of item.children) queue.push(child);
        }
    };

    renderer.updateContainer(element, container, null, callback);
}