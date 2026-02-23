import { http, createConfig } from 'wagmi'
import { mainnet, localhost } from 'wagmi/chains'
import { injected } from 'wagmi/connectors'

export const config = createConfig({
    chains: [mainnet, localhost],
    connectors: [
        injected(),
    ],
    transports: {
        [mainnet.id]: http(),
        [localhost.id]: http(),
    },
})
