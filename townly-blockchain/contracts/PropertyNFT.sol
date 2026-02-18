// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract PropertyNFT is ERC721, Ownable {
    uint256 private _tokenIdCounter;

    // Store metadata URIs manually
    mapping(uint256 => string) private _tokenURIs;

    constructor() ERC721("Townly Property", "TOWNLY") Ownable(msg.sender) {}

    /**
     * @dev Mint a new Property NFT
     * @param to Address receiving the NFT (platform/admin wallet)
     * @param metadataURI Metadata URI (IPFS / server URL)
     */
    function mintPropertyNFT(
        address to,
        string memory metadataURI
    ) external onlyOwner returns (uint256) {
        _tokenIdCounter++;

        uint256 newTokenId = _tokenIdCounter;
        _safeMint(to, newTokenId);
        _tokenURIs[newTokenId] = metadataURI;

        return newTokenId;
    }

    function tokenURI(uint256 tokenId)
        public
        view
        override
        returns (string memory)
    {
        require(ownerOf(tokenId) != address(0), "Token does not exist");
        return _tokenURIs[tokenId];
    }
}
