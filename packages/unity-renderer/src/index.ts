import reconciler from 'react-reconciler';

const renderer = reconciler(__HOSTCONFIG__);

export function render(element: React.ReactNode, callback: () => void = () => {}) {

    const container = renderer.createContainer(__CONTAINER__, false, false);
    renderer.updateContainer(element, container, null, callback);
}

export function useQuery(componentTypes: string[]) {
    function onUpdate() {

    }

    
}