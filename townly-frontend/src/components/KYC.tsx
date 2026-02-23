import React, { useState } from 'react';
import { UserCheck, Shield, Upload, CheckCircle2, Loader2 } from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../services/api';

const KYC: React.FC = () => {
    const queryClient = useQueryClient();
    const [step, setStep] = useState(1);
    const [formData, setFormData] = useState({
        fullName: '',
        dateOfBirth: '',
        fullAddress: '',
        documentType: 'Passport',
        documentFile: null as File | null,
        selfieFile: null as File | null
    });

    const { data: kycStatus, isLoading: statusLoading } = useQuery({
        queryKey: ['kyc-status'],
        queryFn: async () => {
            const res = await api.get('/api/kyc/me/status');
            return res.data;
        }
    });

    const mutation = useMutation({
        mutationFn: async (vars: FormData) => {
            return await api.post('/api/kyc/submit', vars, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['kyc-status'] });
        }
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const data = new FormData();
        data.append('fullName', formData.fullName);
        data.append('dateOfBirth', formData.dateOfBirth);
        data.append('fullAddress', formData.fullAddress);
        data.append('documentType', formData.documentType);
        if (formData.documentFile) data.append('documentFile', formData.documentFile);
        if (formData.selfieFile) data.append('selfieFile', formData.selfieFile);

        await mutation.mutateAsync(data);
    };

    if (statusLoading) return <div style={{ textAlign: 'center', padding: '100px' }}><Loader2 className="animate-spin" size={48} /></div>;

    if (kycStatus?.status === 'Approved') {
        return (
            <div className="card" style={{ maxWidth: '600px', margin: '0 auto', padding: '60px', textAlign: 'center' }}>
                <div style={{ color: 'var(--primary)', marginBottom: '24px' }}><CheckCircle2 size={64} /></div>
                <h2 className="text-gradient">Verification Verified!</h2>
                <p style={{ color: 'var(--text-secondary)', marginBottom: '32px' }}>Your identity has been successfully verified. You now have full access to invest.</p>
                <button className="btn-premium" style={{ width: '100%' }} onClick={() => window.location.reload()}>Start Investing</button>
            </div>
        );
    }

    if (kycStatus?.status === 'Pending' || mutation.isSuccess) {
        return (
            <div className="card" style={{ maxWidth: '600px', margin: '0 auto', padding: '60px', textAlign: 'center' }}>
                <div style={{ color: 'var(--secondary)', marginBottom: '24px' }}><Loader2 className="animate-spin" size={64} /></div>
                <h2 className="text-gradient">KYC Under Review</h2>
                <p style={{ color: 'var(--text-secondary)', marginBottom: '32px' }}>Our compliance team is currently reviewing your documents. This usually takes 24-48 hours.</p>
                <button className="btn-premium" style={{ width: '100%', background: 'var(--card-bg)', color: '#fff', border: '1px solid var(--card-border)' }} disabled>Awaiting Review</button>
            </div>
        );
    }

    return (
        <div className="card" style={{ maxWidth: '700px', margin: '0 auto', padding: '48px' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 16, marginBottom: '32px' }}>
                <div style={{ padding: '12px', background: 'rgba(16, 185, 129, 0.1)', color: 'var(--primary)', borderRadius: '12px' }}>
                    <UserCheck size={24} />
                </div>
                <div>
                    <h2 style={{ marginBottom: '4px' }}>Identity Verification</h2>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Level 1 Verification - Required for all investment activities.</p>
                </div>
            </div>

            <div style={{ display: 'flex', gap: '8px', marginBottom: '32px' }}>
                {[1, 2, 3].map(s => (
                    <div key={s} style={{ flex: 1, height: '4px', background: step >= s ? 'var(--primary)' : 'var(--card-border)', borderRadius: '2px' }}></div>
                ))}
            </div>

            <form onSubmit={handleSubmit}>
                {step === 1 && (
                    <div className="form-step">
                        <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: '16px', marginBottom: '24px' }}>
                            <div>
                                <label style={{ display: 'block', marginBottom: '8px', fontSize: '12px', color: 'var(--text-muted)' }}>Full Legal Name</label>
                                <input
                                    type="text"
                                    placeholder="Enter your full name"
                                    required
                                    value={formData.fullName}
                                    onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                                    style={{ width: '100%', padding: '14px', background: 'var(--glass-bg)', border: '1px solid var(--card-border)', borderRadius: '12px', color: '#fff', outline: 'none' }}
                                />
                            </div>
                        </div>
                        <div style={{ marginBottom: '24px' }}>
                            <label style={{ display: 'block', marginBottom: '8px', fontSize: '12px', color: 'var(--text-muted)' }}>Date of Birth</label>
                            <input
                                type="date"
                                required
                                value={formData.dateOfBirth}
                                onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })}
                                style={{ width: '100%', padding: '14px', background: 'var(--glass-bg)', border: '1px solid var(--card-border)', borderRadius: '12px', color: '#fff', outline: 'none' }}
                            />
                        </div>
                        <div style={{ marginBottom: '32px' }}>
                            <label style={{ display: 'block', marginBottom: '8px', fontSize: '12px', color: 'var(--text-muted)' }}>Residential Address</label>
                            <textarea
                                placeholder="Enter your full permanent address"
                                required
                                value={formData.fullAddress}
                                onChange={(e) => setFormData({ ...formData, fullAddress: e.target.value })}
                                style={{ width: '100%', padding: '14px', background: 'var(--glass-bg)', border: '1px solid var(--card-border)', borderRadius: '12px', color: '#fff', outline: 'none', minHeight: '100px' }}
                            />
                        </div>
                        <button type="button" className="btn-premium" style={{ width: '100%' }} onClick={() => setStep(2)}>Next Step</button>
                    </div>
                )}

                {step === 2 && (
                    <div className="form-step">
                        <h3 style={{ marginBottom: '20px' }}>Document Upload</h3>
                        <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginBottom: '24px' }}>Please upload a clear photo of your Government-issued ID (Passport or Driver's License).</p>

                        <div style={{ position: 'relative', border: '2px dashed var(--card-border)', borderRadius: '24px', padding: '48px', textAlign: 'center', marginBottom: '24px', cursor: 'pointer', transition: 'var(--transition-smooth)' }} className="hover-primary">
                            <input
                                type="file"
                                style={{ position: 'absolute', inset: 0, opacity: 0, cursor: 'pointer' }}
                                onChange={(e) => setFormData({ ...formData, documentFile: e.target.files?.[0] || null })}
                            />
                            <Upload size={40} style={{ color: 'var(--primary)', marginBottom: '16px' }} />
                            <p style={{ fontWeight: '700', fontSize: '18px' }}>{formData.documentFile ? formData.documentFile.name : 'Upload Identity Document'}</p>
                            <p style={{ color: 'var(--text-muted)', fontSize: '13px', marginTop: '8px' }}>Click to browse or drag and drop Passport/ID</p>
                        </div>

                        <div style={{ position: 'relative', border: '2px dashed var(--card-border)', borderRadius: '24px', padding: '48px', textAlign: 'center', marginBottom: '32px', cursor: 'pointer', transition: 'var(--transition-smooth)' }} className="hover-primary">
                            <input
                                type="file"
                                style={{ position: 'absolute', inset: 0, opacity: 0, cursor: 'pointer' }}
                                onChange={(e) => setFormData({ ...formData, selfieFile: e.target.files?.[0] || null })}
                            />
                            <Shield size={40} style={{ color: 'var(--secondary)', marginBottom: '16px' }} />
                            <p style={{ fontWeight: '700', fontSize: '18px' }}>{formData.selfieFile ? formData.selfieFile.name : 'Take/Upload a Selfie'}</p>
                            <p style={{ color: 'var(--text-muted)', fontSize: '13px', marginTop: '8px' }}>Required for biometric verification</p>
                        </div>

                        <div style={{ display: 'flex', gap: '12px' }}>
                            <button type="button" className="btn-premium" style={{ flex: 1, background: 'transparent', border: '1px solid var(--card-border)', color: '#fff' }} onClick={() => setStep(1)}>Back</button>
                            <button type="button" className="btn-premium" style={{ flex: 1 }} onClick={() => setStep(3)}>Next Step</button>
                        </div>
                    </div>
                )}

                {step === 3 && (
                    <div className="form-step">
                        <h3 style={{ marginBottom: '20px' }}>Declaration</h3>
                        <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginBottom: '24px' }}>By submitting this verification, you agree to our Terms of Service and Privacy Policy regarding data handling.</p>

                        <div style={{ background: 'rgba(255,255,255,0.02)', padding: '20px', borderRadius: '12px', marginBottom: '24px', border: '1px solid var(--card-border)' }}>
                            <div style={{ display: 'flex', gap: '12px', marginBottom: '12px' }}>
                                <input type="checkbox" id="terms" required />
                                <label htmlFor="terms" style={{ fontSize: '13px' }}>I confirm that the information provided is accurate.</label>
                            </div>
                            <div style={{ display: 'flex', gap: '12px' }}>
                                <input type="checkbox" id="privacy" required />
                                <label htmlFor="privacy" style={{ fontSize: '13px' }}>I understand how my documents will be processed.</label>
                            </div>
                        </div>

                        <div style={{ display: 'flex', gap: '12px' }}>
                            <button type="button" className="btn-premium" style={{ flex: 1, background: 'transparent', border: '1px solid var(--card-border)', color: '#fff' }} onClick={() => setStep(2)}>Back</button>
                            <button type="submit" className="btn-premium" style={{ flex: 1 }}>Submit Verification</button>
                        </div>
                    </div>
                )}
            </form>

            <div style={{ marginTop: '40px', display: 'flex', alignItems: 'center', gap: 12, padding: '16px', borderRadius: '12px', background: 'rgba(99, 102, 241, 0.05)', color: 'var(--secondary)', border: '1px solid rgba(99, 102, 241, 0.1)' }}>
                <Shield size={20} />
                <p style={{ fontSize: '13px' }}>Your data is encrypted and stored securely using AES-256 standards.</p>
            </div>
        </div>
    );
};

export default KYC;
