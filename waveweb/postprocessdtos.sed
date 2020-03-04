s/^import \(.\+\) from "\(.\+\)";/import \1 from \'\2\';/g
s/sampleNoFeature: number\[\];/sampleNoFeature: number[][];/g
s/sampleHighFrequency: number\[\];/sampleHighFrequency: number[][];/g
s/sampleLowFrequency: number\[\];/sampleLowFrequency: number[][];/g
