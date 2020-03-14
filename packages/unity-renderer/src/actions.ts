

export type Entity = [string, Object];

export function createEntity(components: Entity[]) {
    global.bridge.sendBatched({
        operation: 'create-entity-with-components',
        data: JSON.stringify({ components: components.map(([name, props]) => ({
            name,
            props: JSON.stringify(props)
        }))})
    })
}

export function removeEntity(index: number, components: string[]) {
    global.bridge.sendBatched({
        operation: 'remove-entity',
        index,
        components
    });
}