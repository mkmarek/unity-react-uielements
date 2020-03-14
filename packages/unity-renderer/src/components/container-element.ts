import Element from './element';

export default class ContainerElement extends Element {
    public updates: any[];

    constructor(type: string, props: any) {
        super(type, props, null);
    }

    sendUpdate(update) {
        if (!this.updates) this.updates = [];

        this.updates.push(update);
    }

    commitUpdates() {
        const noDuplicateUpdates = [];

        for (let update of this.updates) {
            if (update.operation == 'remove-child') {
                if (this.updates.find(e =>
                    e.operation === 'add-child' &&
                    e.parent === update.parent &&
                    e.child === update.child)) {
                        continue;
                }
            }

            if (update.operation == 'add-child') {
                if (this.updates.find(e =>
                    e.operation === 'remove-child' &&
                    e.parent === update.parent &&
                    e.child === update.child)) {
                        continue;
                }
            }

            noDuplicateUpdates.push(update);
        }

        if (noDuplicateUpdates.length) {
            global.natives.jsToNativeBridge(JSON.stringify({ messages: noDuplicateUpdates }));
        }
        this.updates = [];
    }
}