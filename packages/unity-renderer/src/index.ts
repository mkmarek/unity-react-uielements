import reconciler from 'react-reconciler/persistent';

const renderer = reconciler(__HOSTCONFIG__);

export function render(element: React.ReactNode, callback: () => void = () => {}) {

    const container = renderer.createContainer(__CONTAINER__, false, false);

    console.log('Have container');
    renderer.updateContainer(element, container, null, callback);
}