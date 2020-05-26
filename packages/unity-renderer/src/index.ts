import { useState, useEffect } from 'react';
import reconciler from 'react-reconciler';

const renderer = reconciler(__HOSTCONFIG__);

export function render(element: React.ReactNode, callback: () => void = () => {}) {

    const container = renderer.createContainer(__CONTAINER__, false, false);
    renderer.updateContainer(element, container, null, callback);
}

const registeredQueries = [];
const registeredQueryCallbacks = [];

export function useQuery(componentTypes: string[]) {
    const [queryIndex, setQueryIndex] = useState(-1);
    const [_, setVersion] = useState(0);

    if (queryIndex >= 0) {
        registeredQueryCallbacks[queryIndex] = () => {
            setVersion(registeredQueries[queryIndex].getVersion());
        }
    }

    useEffect(() => {
        return () => {
            registeredQueries[queryIndex].dispose();
        }
    }, componentTypes)

    if (queryIndex < 0) {
        const nextQueryIndex = registeredQueries.length;
        setQueryIndex(nextQueryIndex);
        const createdQuery = __CREATEQUERY__(componentTypes, () => {
            registeredQueryCallbacks[nextQueryIndex]()
        });
        registeredQueries.push(createdQuery);

        return createdQuery;
    } else {
        return registeredQueries[queryIndex];
    }
}