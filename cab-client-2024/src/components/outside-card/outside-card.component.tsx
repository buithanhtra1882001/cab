import React, { PropsWithChildren, useRef, useState } from 'react';
import { useOnClickOutside } from 'usehooks-ts';

interface OutsideCardProps extends PropsWithChildren {
  defaultOpen?: boolean;
}

const OutsideCard = ({ defaultOpen, children }: OutsideCardProps) => {
  const cardRef = useRef<HTMLDivElement>(null);

  const [visible, setVisible] = useState<boolean>(!!defaultOpen);

  useOnClickOutside(cardRef, () => {
    setVisible(false);
  });
  return <div ref={cardRef}>{visible ? children : null}</div>;
};

export default OutsideCard;
