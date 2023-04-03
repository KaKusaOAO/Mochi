// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('main.js', {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    }
});

setModuleImports("utils.js", {
    getGlobal: () => window,
    get: (obj, prop) => obj[prop],
    set: (obj, prop, value) => obj[prop] = value,
    invoke: (obj, args) => {
        let func = obj;
        if (!func) return;
        if (typeof func.func === "function") func = func.func;

        let r = args.map(a => {
            if (typeof a.func === "function") return a.func;
            return a;
        });
        return obj(...r);
    },
    getInvoke: (obj, prop, args) => { 
        let func = obj[prop];
        let r = args.map(a => {
            if (typeof a.func === "function") return a.func;
            return a;
        });
        return func.apply(obj, r);
    },
    convertToAny: (obj) => {
        if (typeof obj === "function") {
            return {func: obj};
        }
        
        return obj;
    },
    pass: (obj) => obj
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
await dotnet.run();