import React from 'react';
import { IVirtualListContentProps } from './virtual-list-content.type';

const classNamePrefix = 'tt-virtual-list-content-component';

export const VirtualListContent: React.FC<IVirtualListContentProps> = (props) => {
  const { children } = props;
  return (
    <div className={`${classNamePrefix} h-auto`}>
      <div className="border-t-1 border-b-1 min-h-200 h-screen">
        <div className="virtual-scroll-root" style={{ overflowY: 'auto', height: 'inherit' }}>
          <div className="virtual-scroll-wrapper" style={{ padding: '0px' }}>
            <div className="virtual-scroll-item">{children}</div>
          </div>
          <div className="virtual-scroll-item">
            <div slot="footer" />
          </div>
          <div className="shepherd" style={{ width: '100%', height: '0px' }} />
        </div>
      </div>
    </div>
  );
};
