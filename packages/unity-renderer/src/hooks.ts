import * as React from 'react';
import guid from './guid';

export function useQuery(queryName: string[]) {
    const [id] = React.useState(guid());
    const [data, setData] = React.useState([]);

    if (!global.bridge.isComponentDataHookRegistered(id)) {
        global.bridge.registerComponentDataHook(id, queryName, setData);
    }

    React.useEffect(() => () => {
            global.bridge.removeComponentDataHook(id);
    }, []);

    if (!data.length) return [true];

    const result: any[] = [false];

    for (let i = 0; i < data.length; i++) {
        const componentData = data[i];
        const update = (index: number, payload: any) => {
            global.bridge.updateComponentDataViaHook(
                id, i, index, { ...componentData[index], ...payload });
        };

        result.push(componentData);
        result.push(update);
    }

    return result;
}