import guid from '../guid';
import ContainerElement from './container-element';

const mapProp = (prop) => prop && typeof prop === 'function' ? true : prop;

// TODO: make better mapping to avoid weird props being passed in
const filterProps = (props) =>
    Object.keys(props).reduce((p, c) => c !== 'children' ? ({ ...p, [c]: mapProp(props[c]) }) : p, {});

export default class Element {
    public props: any;
    public id: string;
    public children: Element[];

    public rootContainer: ContainerElement;

    constructor(type: string, props: any, rootContainer: ContainerElement, id = null) {
        this.id = id || guid();
        this.props = props;
        this.children = [];
        this.rootContainer = rootContainer;

        if (!id) {
            this.sendUpdate({
                operation: 'create',
                type,
                isContainer: rootContainer == null,
                id: this.id,
                props: filterProps(props)
            });
        }
    }

    sendUpdate(update) {
        this.rootContainer.updates.push(update);
    }

    addChild(child: Element) {
        this.sendUpdate({
            operation: 'add-child',
            parent: this.id,
            child: child.id
        });
        this.children.push(child);
    }

    removeChild(child: Element) {
        this.sendUpdate({
            operation: 'remove-child',
            parent: this.id,
            child: child.id
        });
        this.children = this.children.filter(e => e !== child);
    }

    insertBefore(child: Element, beforeChild: Element) {
        const index = this.children.findIndex((el) => el.id == beforeChild.id);

        if (index >= 0) {
            this.insertAtIndex(child, index);
        } else {
            throw new Error('No element in array');
        }
    }

    insertAtIndex(child: Element, index: number) {
        this.sendUpdate({
            operation: 'insert-child',
            parent: this.id,
            child: child.id,
            index: index
        });

        this.children.splice(index, 0, child);
    }

    updateProps(props: any, shouldSendUpdate = true) {

        if (shouldSendUpdate) {
            this.sendUpdate({
                operation: 'update-props',
                id: this.id,
                props: filterProps(props)
            });
        }

        this.props = { ...this.props, ...props };
    }
}