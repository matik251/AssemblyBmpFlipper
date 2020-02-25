# AssemblyBmpFlipper <img align="right" width="400" height="220" src="https://github.com/matik251/AssemblyBmpFlipper/blob/master/window.png">

Program to learn vector instructions in Assembly and compare time results with C++.
Developed for amd64 architecture- program uses AVX512 fast vector operations in assemlby mode.

## Structure
* Main program with GUI in C# with WPF
* C++ dll for flipping images
* Assembly dll for flipping images

## Assembly instrucions
| Instruction | Purpose |
|-------------|---------|
| MOVQ xmm1, xmm2/m64 | load data to register |
| MOVLHPS xmm1, xmm2 | move low half of register xmm1 to xmm2 |
| MOVDQA xmm1, xmm2 | save data to xmm1 |
| VPSHUFB xmm1, xmm2, xmm3 | flip pixels bytes according to mask in xmm3 (picture)|

<p align="center">
  <img width="300" height="200" src="https://github.com/matik251/AssemblyBmpFlipper/blob/master/vpshufb.png">
</p>

## Shortcut
Main program takes user input and prefeances, loads image and using right library flips image and shows time taken for logic operations. Both picutres are visible for user, output picture is saved to directory.
