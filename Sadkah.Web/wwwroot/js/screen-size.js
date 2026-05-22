const getBreakpoint = width => {
    if (width >= 1400) {
        return "xxl";
    }

    if (width >= 1200) {
        return "xl";
    }

    if (width >= 992) {
        return "lg";
    }

    if (width >= 768) {
        return "md";
    }

    if (width >= 576) {
        return "sm";
    }

    return "xs";
};

export const getScreenSize = () => {
    const width = window.innerWidth || document.documentElement.clientWidth || 0;
    const height = window.innerHeight || document.documentElement.clientHeight || 0;

    return {
        width,
        height,
        breakpoint: getBreakpoint(width)
    };
};
