datablock CameraData(Observer) {};

datablock PlayerData(Monster) {
   shapeFile = "art/monster.dae";
   mass = 90;
   swimForce = 90 * 5;
   maxUnderwaterForwardSpeed = 25;
   maxUnderwaterBackwardSpeed = 25;
   maxUnderwaterSideSpeed = 25;
   groundImpactMinSpeed = 100;
};

datablock PlayerData(Tourist) {
   class = Person;
   shapeFile = "art/tourist.dae";
   mass = 90;
   runSurfaceAngle = 85;
   runForce = 90 * 12;
   maxForwardSpeed = 8;
   maxBackwardSpeed = 8;
   maxSideSpeed = 8;
   swimForce = 90 * 1;
   maxUnderwaterForwardSpeed = 3;
   maxUnderwaterBackwardSpeed = 3;
   maxUnderwaterSideSpeed = 3;
};

datablock PlayerData(Ranger : Tourist) {
   shapeFile = "art/ranger.dae";
};

datablock ParticleData(AttackRippleParticle) {
   textureName = "art/wake";
   dragCoefficient = 0.0;
   gravityCoefficient = 0.0;
   inheritedVelFactor = 0.0;
   lifetimeMS = 4000;
   lifetimeVarianceMS = 1000;
   windCoefficient = 0.0;
   useInvAlpha = true;
   spinRandomMin = 30.0;
   spinRandomMax = 30.0;
   spinSpeed = 0;

   animateTexture = true;
   framesPerSec = 1;
   animTexTiling = "2 1";
   animTexFrames = "0 1";

   colors[0] = "0.7 0.8 1.0 1.0";
   colors[1] = "0.7 0.8 1.0 1.0";
   colors[2] = "0.7 0.8 1.0 0.0";

   sizes[0] = 2.0;
   sizes[1] = 14.0;
   sizes[2] = 22.0;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;
};

datablock particleData(BubbleRippleParticle) {
   textureName = "art/wake";
   dragCoefficient = 0.0;
   gravityCoefficient = 0.0;
   inheritedVelFactor = 0.0;
   lifetimeMS = 2500;
   lifetimeVarianceMS = 200;
   windCoefficient = 0.0;
   useInvAlpha = true;
   spinRandomMin = 30.0;
   spinRandomMax = 30.0;
   spinSpeed = 0;

   animateTexture = true;
   framesPerSec = 1;
   animTexTiling = "2 1";
   animTexFrames = "0 1";

   colors[0] = "1 1 1 1.0";
   colors[1] = "1 1 1 0.8";
   colors[2] = "1 1 1 0.0";

   sizes[0] = 2.0;
   sizes[1] = 8.0;
   sizes[2] = 12.0;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;
};

datablock ParticleData(BubbleParticle)
{
   textureName          = "art/bubble";
   dragCoefficient      = 0.0;
   windCoefficient      = 0.0;
   gravityCoefficient   = -0.1;
   inheritedVelFactor   = 0.00;
   lifetimeMS           = 1600;
   lifetimeVarianceMS   = 500;
   useInvAlpha          = false;

   colors[0]     = "0.7 0.7 0.7 1";
   colors[1]     = "0.7 0.7 0.7 1";
   colors[2]     = "0.7 0.7 0.7 1";
   colors[3]     = "0.7 0.7 0.7 0";

   sizes[0]      = 0.4;
   sizes[1]      = 0.5;
   sizes[2]      = 0.55;
   sizes[3]      = 0.55;

   times[0]      = 0.0;
   times[1]      = 0.5;
   times[2]      = 0.98;
   times[3]      = 1.0;
};

datablock ParticleData(AttackSplashParticle) {
   textureName          = "art/smoke";
   dragCoefficient      = 0.0;
   windCoefficient      = 0.0;
   gravityCoefficient   = 1;
   inheritedVelFactor   = 0.00;
   lifetimeMS           = 3000;
   lifetimeVarianceMS   = 500;
   useInvAlpha          = false;

   colors[0]     = "0.7 0.7 0.7 1";
   colors[1]     = "0.7 0.7 0.7 1";
   colors[2]     = "0.7 0.7 0.7 1";
   colors[3]     = "0.7 0.7 0.7 0";

   sizes[0]      = 6;
   sizes[1]      = 9;
   sizes[2]      = 10.5;
   sizes[3]      = 10.5;

   times[0]      = 0.0;
   times[1]      = 0.5;
   times[2]      = 0.98;
   times[3]      = 1.0;
};

datablock ParticleData(AttackJetParticle : AttackSplashParticle) {
   textureName          = "art/splash";
   gravityCoefficient   = 2;
   lifetimeMS           = 3500;
   lifetimeVarianceMS   = 100;
   spinRandomMin = 00.0;
   spinRandomMax = 90.0;
   spinSpeed = 0.5;

   colors[0]     = "0.7 0.7 0.7 0.7";
   colors[1]     = "0.7 0.7 0.7 0.6";
   colors[2]     = "0.7 0.7 0.7 0.4";
   colors[3]     = "0.7 0.7 0.7 0";

   sizes[0]      = 3;
   sizes[1]      = 5;
   sizes[2]      = 6.5;
   sizes[3]      = 6.5;
};

datablock ParticleData(WakeParticle) {
   textureName          = "art/smoke";
   dragCoefficient      = 0.3;
   gravityCoefficient   = 0;
   inheritedVelFactor   = 0.3;
   lifetimeMS           = 3000;
   lifetimeVarianceMS   = 250;
   useInvAlpha          = true;
   spinRandomMin        = -30.0;
   spinRandomMax        = 30.0;

   colors[0] = "0.7 0.8 1.0 0.1";
   colors[1] = "0.7 0.8 1.0 0.3";
   colors[2] = "0.7 0.8 1.0 0.0";

   sizes[0]      = 1;
   sizes[1]      = 3.75;
   sizes[2]      = 7.5;

   times[0]      = 0.0;
   times[1]      = 0.3;
   times[2]      = 1.0;
};

datablock ParticleEmitterData(BubbleEmitter)
{
   ejectionPeriodMS = 150;
   periodVarianceMS = 0;

   ejectionVelocity = 0.50;
   velocityVariance = 0.00;
   ejectionOffset   = 0.0;

   thetaMin         = 1.0;
   thetaMax         = 100.0;

   particles        = BubbleParticle;
};

datablock ParticleEmitterData(AttackJetEmitter) {
   ejectionPeriodMS = 20;
   periodVarianceMS = 0;

   ejectionVelocity = 14;
   velocityVariance = 2;
   ejectionOffset   = 0;

   thetaMin         = 1.0;
   thetaMax         = 10.0;

   particles = AttackJetParticle;
};

datablock ParticleEmitterData(AttackSplashEmitter) {
   ejectionPeriodMS = 15;
   periodVarianceMS = 0;

   ejectionVelocity = 7;
   velocityVariance = 2;
   ejectionOffset   = 0;

   thetaMin         = 1.0;
   thetaMax         = 50.0;

   particles = AttackSplashParticle;
};

datablock ParticleEmitterData(AttackRippleEmitter) {
   lifetimeMS = 100;
   ejectionPeriodMS = 200;
   periodVarianceMS = 10;
   ejectionVelocity = 0;
   velocityVariance = 0;
   ejectionOffset = 0;
   thetaMin = 89;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 1;
   alignParticles = 1;
   alignDirection = "0 0 1";
   particles = AttackRippleParticle;
};

datablock ParticleEmitterData(WakeEmitter : AttackRippleEmitter) {
   particles = WakeParticle;
};

datablock ParticleEmitterData(BubbleRippleEmitter : AttackRippleEmitter) {
   particles = BubbleRippleParticle;
};

datablock ParticleEmitterNodeData(DefaultNode) {
   timeMultiple = 1;
};
